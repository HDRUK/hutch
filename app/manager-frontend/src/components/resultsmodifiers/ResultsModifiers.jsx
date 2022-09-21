import {
  Heading,
  VStack,
  Grid,
  GridItem,
  Text,
  Button,
  HStack,
  useDisclosure,
  Alert,
  AlertIcon,
} from "@chakra-ui/react";
import { DragDropContext, Draggable, Droppable } from "react-beautiful-dnd";
import { useBackendApi } from "contexts/BackendApi";
import { useActivitySourceResultsModifiersList } from "api/activitysources";
import { useState } from "react";
import {
  FaGripVertical,
  FaTrash,
  FaEdit,
  FaPlus,
  FaInfoCircle,
} from "react-icons/fa";
import { ConfigureResultsModifierModal } from "./ConfigureResultsModifierModal";
import { DeleteModal } from "components/DeleteModal";

export const ResultsModifiers = ({ id }) => {
  const { resultsmodifier, activitysource } = useBackendApi();
  const { data, mutate } = useActivitySourceResultsModifiersList(id);
  const [selected, setSelected] = useState();
  const onDragEnd = async (result) => {
    // dropped outside the list
    if (!result.destination) {
      return;
    }
    const reordered = Array.from(data);
    const [removed] = reordered.splice(result.source.index, 1);
    reordered.splice(result.destination.index, 0, removed);
    mutate(reordered, false);
    await resultsmodifier.putOrder({
      position: result.destination.index + 1,
      id: result.draggableId,
    });
    await mutate();
  };

  const {
    isOpen: isDeleteOpen,
    onOpen: onDeleteOpen,
    onClose: onDeleteClose,
  } = useDisclosure();
  const {
    isOpen: isUpdateOpen,
    onOpen: onUpdateOpen,
    onClose: onUpdateClose,
  } = useDisclosure();

  const onDelete = async (id) => {
    await resultsmodifier.delete({ id: id });
    await mutate();
    setSelected(undefined);
    onDeleteClose();
  };

  const closeDelete = () => {
    onDeleteClose();
    setSelected(undefined);
  };
  const closeUpdate = () => {
    onUpdateClose();
    setSelected(undefined);
  };
  const onClickUpdate = (item) => {
    setSelected(item);
    onUpdateOpen();
  };
  return (
    <VStack
      w="100%"
      align="stretch"
      p={4}
      pb={10}
      bg="whiteAlpha"
      borderColor="gray.300"
      h="100%"
      spacing={4}
      display="contents"
    >
      <HStack p={2}>
        <Heading size="lg">Result Modifiers</Heading>
        <Button
          colorScheme="green"
          leftIcon={<FaPlus />}
          onClick={onUpdateOpen}
        >
          <ConfigureResultsModifierModal
            isOpen={isUpdateOpen}
            onClose={closeUpdate}
            action={
              selected ? resultsmodifier.update : activitysource.createModifier
            }
            initialData={selected}
            mutate={mutate}
            activitySourceId={id}
          />
          <Text
            textTransform={"uppercase"}
            fontWeight={700}
            fontSize={"sm"}
            letterSpacing={1.1}
          >
            {" "}
            New
          </Text>
        </Button>
      </HStack>
      <Grid
        templateAreas={`"header header"
                "nav main"
                "nav footer"`}
        templateColumns="repeat(1,1fr)"
        gap={1}
        display={"grid"}
      >
        <DragDropContext onDragEnd={onDragEnd}>
          <Droppable droppableId="droppable">
            {(provided, snapshot) => (
              <div {...provided.droppableProps} ref={provided.innerRef}>
                {data.length > 0 ? (
                  <>
                    <Grid
                      templateColumns={"repeat(4,1fr)"}
                      display={"grid"}
                      p={3}
                    >
                      <GridItem colStart={1} colSpan={1} area={"nav"} pl={2}>
                        <Text fontWeight={"bold"}> Order </Text>
                      </GridItem>
                      <GridItem colStart={2} colSpan={1} area={"nav"} pl={5}>
                        <Text fontWeight={"bold"}> Type </Text>
                      </GridItem>
                      <GridItem colStart={3} colSpan={1} area={"nav"} pl={5}>
                        <Text fontWeight={"bold"}> Parameters </Text>
                      </GridItem>
                      <GridItem
                        colStart={4}
                        colSpan={1}
                        area={"nav"}
                        pl={5}
                      ></GridItem>
                    </Grid>

                    {data
                      .sort((item, index) => item.order - index.order)
                      .map((item, index) => (
                        <Draggable
                          key={item.id}
                          draggableId={`${item.id}`}
                          index={index}
                        >
                          {(provided, snapshot) => (
                            <div
                              ref={provided.innerRef}
                              {...provided.draggableProps}
                              {...provided.dragHandleProps}
                            >
                              <Grid
                                templateColumns={"repeat(4,1fr)"}
                                display={"grid"}
                                pb={2}
                                borderRadius={5}
                                borderWidth={1}
                                borderColor={
                                  snapshot.isDragging ? "blue.500" : "blue.10"
                                }
                                bg={snapshot.isDragging ? "blue.50" : "gray.50"}
                                alignItems="center"
                              >
                                <GridItem
                                  colStart={1}
                                  colSpan={1}
                                  pl={2}
                                  pt={1}
                                  display="flex"
                                  alignItems="center"
                                >
                                  <FaGripVertical />
                                  <Text pl={2}> {item.order} </Text>
                                </GridItem>

                                <GridItem
                                  colStart={2}
                                  colSpan={1}
                                  pl={5}
                                  display="grid"
                                >
                                  <Text p={"1"}>{item.type.id}</Text>
                                </GridItem>
                                {Object.keys(item.parameters).map(
                                  (key, value) => (
                                    <GridItem
                                      colStart={3}
                                      colSpan={1}
                                      pl={5}
                                      display="grid"
                                    >
                                      <Text p={"1"}>
                                        {key}: {item.parameters[key]}
                                      </Text>
                                    </GridItem>
                                  )
                                )}
                                <GridItem
                                  colStart={4}
                                  colSpan={1}
                                  pl={5}
                                  display="-ms-inline-grid"
                                >
                                  <Button
                                    style={{ backgroundColor: "transparent" }}
                                  >
                                    {" "}
                                    <FaEdit
                                      color="darkslategray"
                                      onClick={() => onClickUpdate(item)}
                                    />
                                  </Button>
                                  <Button
                                    style={{ backgroundColor: "transparent" }}
                                    onClick={onDeleteOpen}
                                  >
                                    <DeleteModal
                                      title={`Delete Results Modifier ${
                                        selected ? selected.id : ""
                                      }`}
                                      body="Are you sure you want to delete this results modifier? You will not be able to reverse this"
                                      isOpen={isDeleteOpen}
                                      onClose={closeDelete}
                                      id={selected ? selected.id : undefined}
                                      onDelete={() => onDelete(item.id)}
                                    />{" "}
                                    <FaTrash color="#cf222eed" />
                                  </Button>

                                  <ConfigureResultsModifierModal
                                    isOpen={isUpdateOpen}
                                    onClose={closeUpdate}
                                    action={
                                      selected
                                        ? resultsmodifier.update
                                        : activitysource.createModifier
                                    }
                                    initialData={selected}
                                    mutate={mutate}
                                    activitySourceId={id}
                                  />
                                </GridItem>
                              </Grid>
                            </div>
                          )}
                        </Draggable>
                      ))}
                  </>
                ) : (
                  <HStack p={2}>
                    <Alert status="info">
                      <AlertIcon />
                      No Results Modifiers defined for this ActivitySource.
                    </Alert>
                  </HStack>
                )}
                {provided.placeholder}
              </div>
            )}
          </Droppable>
        </DragDropContext>
      </Grid>
    </VStack>
  );
};
