import {
  Container,
  Heading,
  ListItem,
  UnorderedList,
  VStack,
} from "@chakra-ui/react";
import ReactMarkdown from "react-markdown";
import gfm from "remark-gfm";
import "github-markdown-css";
import { AutoLink } from "components/AutoLink";
import useSWR from "swr";
import ky from "ky";

export const ContentPage = ({ contentKey }) => {
  const { data: content } = useSWR(
    `/locales/dev/${contentKey}.md`,
    async (url) => ky.get(url).text(),
    {
      suspense: true,
    }
  );

  return (
    <Container>
      <VStack align="start" p={2} spacing={4}>
        <ReactMarkdown
          remarkPlugins={[gfm]}
          components={{
            // replace some plain HTML with Chakra components
            // which nets us desirable styling mostly
            a: ({ href, ...props }) => <AutoLink url={href} {...props} />,
            h1: (props) => <Heading size="2xl" {...props} />,
            h2: (props) => <Heading size="xl" {...props} />,
            h3: (props) => <Heading size="lg" {...props} />,
            h4: (props) => <Heading size="md" {...props} />,
            h5: (props) => <Heading size="sm" {...props} />,
            h6: (props) => <Heading size="sm" {...props} />,
            ul: (props) => <UnorderedList pl={8} {...props} />,
            li: (props) => <ListItem {...props} />,
          }}
        >
          {content}
        </ReactMarkdown>
      </VStack>
    </Container>
  );
};
