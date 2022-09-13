import {
  Heading,
  HStack,
  Text,
  LinkBox,
  LinkOverlay,
  Box,
  Divider,
  Center,
  Stack,
  useColorModeValue,
  IconButton,
} from "@chakra-ui/react";
import { FaTrash, FaDesktop, FaLeaf } from "react-icons/fa";
import { Link } from "react-router-dom";

export const ActivitySourceSummary = ({
  title,
  href,
  sourceURL,
  collectionId,
  onDelete,
  ...p
}) => (
  <Center py={4}>
    <LinkBox width={'700px'}>

      <Box
        width={'700px'}

        bg={useColorModeValue('white', 'gray.900')}
        boxShadow={'2xl'}
        rounded={'md'}
        p={6}
        overflow={'hidden'}
        {...p}
        _hover={{
          transform: 'translateY(-2px)',
          boxShadow: 'lg',
        }}

      >
        <LinkOverlay as={Link} to={`${href}`} />
        <Stack>
          <Text
            color={'blue.500'}
            textTransform={'uppercase'}
            fontWeight={800}
            fontSize={'sm'}
            letterSpacing={1.1}>Activity Source
          </Text>
          <Divider />
          <IconButton
            h={5}
            w={5}
            icon={<FaTrash />}
            alignSelf={'flex-end'}
            style={{ background: 'transparent' }}
            color={'red.500'}
            onClick={onDelete} />

          <Heading
            color={useColorModeValue('gray.700', 'white')}
            fontSize={'2xl'}
            fontFamily={'body'}
          >
            {title}

          </Heading>

        </Stack>
        <Stack mt={6} direction={'row'} spacing={4} align={'center'}>

          <Stack direction={'column'} spacing={0} fontSize={'sm'} align='center' display={'block'}>

            {sourceURL && (
              <div style={{ display: 'flex', alignItems: 'center' }}>
                <FaDesktop />
                <Text fontWeight={'700'} p={1} letterSpacing={0.2} color={'blue.500'} textTransform={'uppercase'}>
                  Host:
                </Text>
                <Text>
                  {sourceURL}
                </Text>
              </div>
            )}

            {collectionId && (
              <div style={{ display: 'flex', alignItems: 'center' }}>
                <FaLeaf />
                <Text fontWeight={'700'} p={'1'} letterSpacing={0.2} color={'green.500'} textTransform={'uppercase'}>
                  Resource ID:
                </Text>
                <Text>
                  {collectionId}
                </Text>
              </div>
            )}
          </Stack>
        </Stack>
      </Box>

    </LinkBox >
  </Center >
);
